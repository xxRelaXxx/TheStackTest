import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Degustazione } from './degustazione';

describe('Degustazione', () => {
  let component: Degustazione;
  let fixture: ComponentFixture<Degustazione>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Degustazione]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Degustazione);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
